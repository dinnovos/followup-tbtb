import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { CreateContactRequest } from '../contact.model';
import { Manager, PatientListItem } from '../patient.model';
import { PatientsService } from '../patients.service';

@Component({
  selector: 'app-contact-registration',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './contact-registration.component.html'
})
export class ContactRegistrationComponent implements OnInit {
  patients: PatientListItem[] = [];
  managers: Manager[] = [];
  successMessage: string | null = null;
  errorMessage: string | null = null;
  fieldErrors: Record<string, string[]> = {};
  form!: FormGroup;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly patientsService: PatientsService
  ) {
    this.form = this.formBuilder.group({
      patientId: [null, Validators.required],
      managerId: [null, Validators.required],
      contactDate: ['', Validators.required],
      channel: ['', Validators.required],
      result: ['', Validators.required],
      notes: ['', Validators.maxLength(500)]
    });
  }

  ngOnInit(): void {
    this.patientsService.getPatients().subscribe({
      next: (patients) => (this.patients = patients),
      error: () => (this.errorMessage = 'No se pudo cargar la lista de pacientes.')
    });

    this.patientsService.getManagers().subscribe({
      next: (managers) => (this.managers = managers),
      error: () => (this.errorMessage = 'No se pudo cargar la lista de gestores.')
    });
  }

  submit(): void {
    this.successMessage = null;
    this.errorMessage = null;
    this.fieldErrors = {};

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { patientId, ...request } = this.form.getRawValue();

    this.patientsService.registerContact(patientId, request as CreateContactRequest).subscribe({
      next: () => {
        this.successMessage = 'Contacto registrado correctamente.';
        this.form.reset();
      },
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
