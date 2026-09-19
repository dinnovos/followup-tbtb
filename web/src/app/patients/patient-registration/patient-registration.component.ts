import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { CreatePatientRequest, Manager } from '../patient.model';
import { PatientsService } from '../patients.service';

@Component({
  selector: 'app-patient-registration',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './patient-registration.component.html'
})
export class PatientRegistrationComponent implements OnInit {
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
      registeredByManagerId: [null, Validators.required],
      name: ['', [Validators.required, Validators.maxLength(150)]],
      country: ['', Validators.required],
      documentType: ['', Validators.required],
      documentNumber: ['', [Validators.required, Validators.maxLength(20)]],
      phone: ['', [Validators.required, Validators.pattern(/^\+?[0-9]{7,15}$/)]],
      email: ['', [Validators.email, Validators.maxLength(150)]],
      city: ['', [Validators.required, Validators.maxLength(100)]],
      treatmentStartDate: ['', Validators.required]
    });
  }

  ngOnInit(): void {
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

    const { email, ...rest } = this.form.getRawValue();

    // email es opcional (DataAnnotations [EmailAddress] en el backend valida
    // el contenido si el campo está presente, incluso vacío -- un "" real
    // falla esa validación, distinto de omitir el campo por completo).
    const request: CreatePatientRequest = {
      ...rest,
      ...(email ? { email } : {})
    };

    this.patientsService.registerPatient(request).subscribe({
      next: (patient) => {
        this.successMessage = `Paciente registrado con id ${patient.id}.`;
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
