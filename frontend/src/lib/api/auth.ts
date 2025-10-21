import { http } from './http';
import { IDENTITY_API } from '../env';
import { setToken } from '../auth';

const client = http(IDENTITY_API);

// API’ndeki endpoint ismi farklıysa path’i değiştir: /api/auth/register
export async function register(nickname: string) {
  const { data } = await client.post('/api/auth/register', { nickname });
  // data token string ise:
  const token = typeof data === 'string' ? data : data?.token;
  if (!token) throw new Error('Token alınamadı');
  setToken(token);
  return token;
}