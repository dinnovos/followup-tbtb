import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Channel, ContactResult, CorrectContactRequest } from '../contact.model';
import { channelLabel as getChannelLabel, resultLabel as getResultLabel } from '../contact-labels';
import { Manager } from '../patient.model';
import { PatientsService } from '../patients.service';

@Component({
  selector: 'app-contact-correction',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './contact-correction.component.html'
})
export class ContactCorrectionComponent implements OnInit {
  contactId!: number;
  patientId: number | null = null;
  managers: Manager[] = [];
  contactLoaded = false;
  successMessage: string | null = null;
  errorMessage: string | null = null;
  fieldErrors: Record<string, string[]> = {};
  form: FormGroup;

  // Los 4 campos arrancan con el valor real del contacto (ver ngOnInit), no
  // en blanco -- así que "se corrigió este campo" ya no significa "tiene
  // valor", significa "quedó distinto a como llegó". Estas 4 propiedades son
  // ese punto de comparación -- públicas porque el template las usa para
  // mostrar "antes: X" bajo el campo que el gestor está cambiando.
  originalContactDate = '';
  originalChannel: Channel | '' = '';
  originalResult: ContactResult | '' = '';
  originalNotes = '';

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly patientsService: PatientsService,
    private readonly route: ActivatedRoute
  ) {
    this.form = this.formBuilder.group(
      {
        correctedByManagerId: [null, Validators.required],
        reason: ['', [Validators.required, Validators.maxLength(300)]],
        contactDate: ['', Validators.required],
        channel: ['', Validators.required],
        result: ['', Validators.required],
        notes: ['', Validators.maxLength(500)]
      },
      { validators: (control) => this.atLeastOneFieldToCorrect(control) }
    );
  }

  ngOnInit(): void {
    this.contactId = Number(this.route.snapshot.paramMap.get('id'));
    this.patientId = this.route.snapshot.queryParamMap.has('patientId')
      ? Number(this.route.snapshot.queryParamMap.get('patientId'))
      : null;

    this.patientsService.getManagers().subscribe({
      next: (managers) => (this.managers = managers),
      error: () => (this.errorMessage = 'No se pudo cargar la lista de gestores.')
    });

    if (this.patientId === null) {
      this.errorMessage = 'No se pudo determinar el paciente de este contacto. Vuelve al historial e intenta de nuevo.';
      return;
    }

    this.patientsService.getContactHistory(this.patientId).subscribe({
      next: (contacts) => {
        const current = contacts.find((c) => c.id === this.contactId);
        if (!current) {
          this.errorMessage = 'No se encontró este contacto.';
          return;
        }

        this.originalContactDate = current.contactDate;
        this.originalChannel = current.channel;
        this.originalResult = current.result;
        this.originalNotes = current.notes ?? '';

        this.form.patchValue({
          contactDate: this.originalContactDate,
          channel: this.originalChannel,
          result: this.originalResult,
          notes: this.originalNotes
        });
        this.contactLoaded = true;
      },
      error: () => (this.errorMessage = 'No se pudo cargar la información actual del contacto.')
    });
  }

  // Getters (no propiedades) porque tienen que reevaluarse en cada ciclo de
  // detección de cambios de Angular -- cada tecla que el gestor escribe debe
  // poder prender o apagar el aviso "Modificado" al instante, no solo al
  // enviar el formulario.
  get contactDateChanged(): boolean {
    return this.contactLoaded && this.form.get('contactDate')?.value !== this.originalContactDate;
  }

  get channelChanged(): boolean {
    return this.contactLoaded && this.form.get('channel')?.value !== this.originalChannel;
  }

  get resultChanged(): boolean {
    return this.contactLoaded && this.form.get('result')?.value !== this.originalResult;
  }

  get notesChanged(): boolean {
    return this.contactLoaded && this.form.get('notes')?.value !== this.originalNotes;
  }

  // Acepta también '' porque originalChannel/originalResult valen eso
  // brevemente mientras el contacto todavía no cargó -- el guard
  // "contactLoaded" en los getters *Changed impide que el template llegue a
  // llamar esto con '' en la práctica, pero el tipo debe admitirlo para que
  // el template compile sin recurrir a un cast.
  channelLabel(channel: Channel | ''): string {
    return channel === '' ? '' : getChannelLabel(channel);
  }

  resultLabel(result: ContactResult | ''): string {
    return result === '' ? '' : getResultLabel(result);
  }

  // Espejo de CorrectContactRequest.Validate() en el backend: el 400 real lo
  // sigue decidiendo el API, esto solo evita un viaje de red para el caso
  // obvio. Cada campo cuenta como "corregido" si quedó distinto al valor con
  // el que arrancó, sin importar si el cambio fue a un valor nuevo o a vacío
  // (relevante sobre todo para notes, el único de los 4 donde vacío es un
  // valor real y no solo un estado de carga pendiente).
  private atLeastOneFieldToCorrect(control: AbstractControl): ValidationErrors | null {
    const { contactDate, channel, result, notes } = control.value;
    const hasAnyChange =
      contactDate !== this.originalContactDate ||
      channel !== this.originalChannel ||
      result !== this.originalResult ||
      notes !== this.originalNotes;
    return hasAnyChange ? null : { noFieldsProvided: true };
  }

  submit(): void {
    this.successMessage = null;
    this.errorMessage = null;
    this.fieldErrors = {};

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { correctedByManagerId, reason, contactDate, channel, result, notes } = this.form.getRawValue();

    // Solo los campos que quedaron distintos al valor original viajan en el
    // request -- el backend distingue "ausente" (no tocar) de "presente"
    // (sí tocar, aunque el valor nuevo sea vacío, como puede pasar con notes).
    const request: CorrectContactRequest = {
      correctedByManagerId,
      reason,
      ...(contactDate !== this.originalContactDate ? { contactDate } : {}),
      ...(channel !== this.originalChannel ? { channel } : {}),
      ...(result !== this.originalResult ? { result } : {}),
      ...(notes !== this.originalNotes ? { notes } : {})
    };

    this.patientsService.correctContact(this.contactId, request).subscribe({
      next: () => (this.successMessage = 'Contacto corregido correctamente.'),
      error: (response) => {
        if (response.error?.errors) {
          this.fieldErrors = response.error.errors;
        } else {
          this.errorMessage = response.error?.detail ?? 'Ocurrió un error inesperado.';
        }
      }
    });
  }
}
