import { Routes } from '@angular/router';

import { ContactRegistrationComponent } from './patients/contact-registration/contact-registration.component';
import { PatientRegistrationComponent } from './patients/patient-registration/patient-registration.component';

export const routes: Routes = [
  { path: '', component: PatientRegistrationComponent },
  { path: 'contactos/nuevo', component: ContactRegistrationComponent }
];
