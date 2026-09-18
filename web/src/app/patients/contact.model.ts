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
