import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { CreatePatientRequest, Manager } from '../patient.model';
import { PatientsService } from '../patients.service';

@Component({
  selector: 'app-patient-registration',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
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
      phone: ['', [Validators.required, Validators.maxLength(20)]],
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

    const request = this.form.getRawValue() as CreatePatientRequest;

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
