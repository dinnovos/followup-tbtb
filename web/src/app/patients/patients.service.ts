import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Contact, ContactWithHistory, CorrectContactRequest, CreateContactRequest } from './contact.model';
import { CreatePatientRequest, Manager, Patient, PatientListItem } from './patient.model';

const API_URL = 'http://localhost:5129/api';

@Injectable({ providedIn: 'root' })
export class PatientsService {
  constructor(private readonly http: HttpClient) {}

  getManagers(): Observable<Manager[]> {
    return this.http.get<Manager[]>(`${API_URL}/managers`);
  }

  getPatients(): Observable<PatientListItem[]> {
    return this.http.get<PatientListItem[]>(`${API_URL}/patients`);
  }

  registerPatient(request: CreatePatientRequest): Observable<Patient> {
    return this.http.post<Patient>(`${API_URL}/patients`, request);
  }

  registerContact(patientId: number, request: CreateContactRequest): Observable<Contact> {
    return this.http.post<Contact>(`${API_URL}/patients/${patientId}/contacts`, request);
  }

  getContactHistory(patientId: number): Observable<ContactWithHistory[]> {
    return this.http.get<ContactWithHistory[]>(`${API_URL}/patients/${patientId}/contacts`);
  }

  correctContact(contactId: number, request: CorrectContactRequest): Observable<Contact> {
    return this.http.put<Contact>(`${API_URL}/contacts/${contactId}`, request);
  }
}
