import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CreatePatientRequest, Manager, Patient } from './patient.model';

const API_URL = 'http://localhost:5129/api';

@Injectable({ providedIn: 'root' })
export class PatientsService {
  constructor(private readonly http: HttpClient) {}

  getManagers(): Observable<Manager[]> {
    return this.http.get<Manager[]>(`${API_URL}/managers`);
  }

  registerPatient(request: CreatePatientRequest): Observable<Patient> {
    return this.http.post<Patient>(`${API_URL}/patients`, request);
  }
}
