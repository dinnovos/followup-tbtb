import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Channel, ContactResult, ContactWithHistory } from '../contact.model';
import { channelLabel as getChannelLabel, resultLabel as getResultLabel } from '../contact-labels';
import { PatientListItem } from '../patient.model';
import { PatientsService } from '../patients.service';

interface FieldDiff<T> {
  changed: boolean;
  before: T;
  after: T;
}

interface CorrectionDiff {
  contactDate: FieldDiff<string>;
  channel: FieldDiff<Channel>;
  result: FieldDiff<ContactResult>;
  notes: FieldDiff<string | null>;
}

@Component({
  selector: 'app-patient-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './patient-detail.component.html'
})
export class PatientDetailComponent implements OnInit {
  patients: PatientListItem[] = [];
  selectedPatientId: number | null = null;
  contacts: ContactWithHistory[] = [];
  errorMessage: string | null = null;
  expandedContactId: number | null = null;

  constructor(
    private readonly patientsService: PatientsService,
    private readonly route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.patientsService.getPatients().subscribe({
      next: (patients) => {
        this.patients = patients;

        // Si llegamos de vuelta desde "Corregir contacto" con ?patientId=X,
        // reabre el mismo paciente en vez de dejar el selector vacío.
        const patientIdParam = this.route.snapshot.queryParamMap.get('patientId');
        if (patientIdParam) {
          this.selectedPatientId = Number(patientIdParam);
          this.loadHistory();
        }
      },
      error: () => (this.errorMessage = 'No se pudo cargar la lista de pacientes.')
    });
  }

  onPatientChange(): void {
    this.expandedContactId = null;
    this.loadHistory();
  }

  toggleHistory(contactId: number): void {
    this.expandedContactId = this.expandedContactId === contactId ? null : contactId;
  }

  channelLabel(channel: Channel): string {
    return getChannelLabel(channel);
  }

  resultLabel(result: ContactResult): string {
    return getResultLabel(result);
  }

  // El backend guarda, en cada corrección, la fotografía completa de los 4
  // campos -- tenga o no sentido cambiarlos todos a la vez. Para mostrar solo
  // lo que un gestor realmente tocó, comparamos ese "antes" contra el "después":
  // el valor actual del contacto si es la corrección más reciente, o el "antes"
  // de la corrección inmediatamente más reciente que ella si no lo es.
  correctionDiff(contact: ContactWithHistory, index: number): CorrectionDiff {
    const correction = contact.corrections[index];
    const after =
      index === 0
        ? { contactDate: contact.contactDate, channel: contact.channel, result: contact.result, notes: contact.notes }
        : {
            contactDate: contact.corrections[index - 1].previousContactDate,
            channel: contact.corrections[index - 1].previousChannel,
            result: contact.corrections[index - 1].previousResult,
            notes: contact.corrections[index - 1].previousNotes
          };

    return {
      contactDate: {
        changed: correction.previousContactDate !== after.contactDate,
        before: correction.previousContactDate,
        after: after.contactDate
      },
      channel: {
        changed: correction.previousChannel !== after.channel,
        before: correction.previousChannel,
        after: after.channel
      },
      result: {
        changed: correction.previousResult !== after.result,
        before: correction.previousResult,
        after: after.result
      },
      notes: {
        changed: (correction.previousNotes ?? '') !== (after.notes ?? ''),
        before: correction.previousNotes,
        after: after.notes
      }
    };
  }

  private loadHistory(): void {
    this.contacts = [];
    this.errorMessage = null;

    if (this.selectedPatientId === null) {
      return;
    }

    this.patientsService.getContactHistory(this.selectedPatientId).subscribe({
      next: (contacts) => (this.contacts = contacts),
      error: () => (this.errorMessage = 'No se pudo cargar el historial de contactos.')
    });
  }
}
