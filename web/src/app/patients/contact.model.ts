export type Channel = 'Call' | 'WhatsApp' | 'Email';

export type ContactResult = 'Answered' | 'NotAnswered' | 'DeclinedFollowUp';

export interface CreateContactRequest {
  managerId: number;
  contactDate: string;
  channel: Channel;
  result: ContactResult;
  notes?: string;
}

export interface Contact {
  id: number;
  patientId: number;
  managerId: number;
  contactDate: string;
  channel: Channel;
  result: ContactResult;
  notes: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface CorrectContactRequest {
  correctedByManagerId: number;
  reason: string;
  contactDate?: string;
  channel?: Channel;
  result?: ContactResult;
  notes?: string;
}

export interface ContactCorrection {
  correctedByManagerName: string;
  reason: string;
  correctionDate: string;
  previousContactDate: string;
  previousChannel: Channel;
  previousResult: ContactResult;
  previousNotes: string | null;
}

// Lo que devuelve la "consulta con criterio" (GET /api/patients/{patientId}/contacts):
// un Contact igual al de arriba, más su historial de correcciones, si tiene.
export interface ContactWithHistory extends Contact {
  corrections: ContactCorrection[];
}
