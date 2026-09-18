import { Routes } from '@angular/router';

import { ContactCorrectionComponent } from './patients/contact-correction/contact-correction.component';
import { ContactRegistrationComponent } from './patients/contact-registration/contact-registration.component';
import { PatientDetailComponent } from './patients/patient-detail/patient-detail.component';
import { PatientRegistrationComponent } from './patients/patient-registration/patient-registration.component';

export const routes: Routes = [
  { path: '', component: PatientRegistrationComponent },
  { path: 'contactos/nuevo', component: ContactRegistrationComponent },
  { path: 'contactos/:id/corregir', component: ContactCorrectionComponent },
  { path: 'pacientes/historial', component: PatientDetailComponent }
];
