import { http } from './http';
import { CHAT_API } from '../env';

const client = http(CHAT_API);

// Path’ler projendeki Controller’a göre değişebilir: /api/messages/recent ve /api/messages/send varsayımı
export async function getRecent(limit = 50) {
  const { data } = await client.get('/api/messages/recent', { params: { limit } });
  return data as any[];
}

export async function sendMessage(text: string) {
  const { data } = await client.post('/api/messages/send', { text });
  return data;
}