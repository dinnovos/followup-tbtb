export type Country = 'Colombia' | 'Peru' | 'Ecuador';

export type DocumentType = 'CC' | 'DNI' | 'CI' | 'Passport';

export interface Manager {
  id: number;
  name: string;
}

export interface CreatePatientRequest {
  registeredByManagerId: number;
  name: string;
  country: Country;
  documentType: DocumentType;
  documentNumber: string;
  phone: string;
  email?: string;
  city: string;
  treatmentStartDate: string;
}

export interface Patient {
  id: number;
  registeredByManagerId: number;
  country: Country;
  documentType: DocumentType;
  documentNumber: string;
  name: string;
  phone: string;
  email: string | null;
  city: string;
  treatmentStartDate: string;
  createdAt: string;
}

export interface PatientListItem {
  id: number;
  name: string;
  documentNumber: string;
}
