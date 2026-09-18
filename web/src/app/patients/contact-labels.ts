import { Channel, ContactResult } from './contact.model';

const CHANNEL_LABELS: Record<Channel, string> = {
  Call: 'Llamada',
  WhatsApp: 'WhatsApp',
  Email: 'Correo'
};

const RESULT_LABELS: Record<ContactResult, string> = {
  Answered: 'Contestó',
  NotAnswered: 'No contestó',
  DeclinedFollowUp: 'Rechazó seguimiento'
};

export function channelLabel(channel: Channel): string {
  return CHANNEL_LABELS[channel];
}

export function resultLabel(result: ContactResult): string {
  return RESULT_LABELS[result];
}
