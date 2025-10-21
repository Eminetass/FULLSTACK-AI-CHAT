import axios from 'axios';
import { getToken } from '../auth';

export function http(baseURL: string) {
  const client = axios.create({ baseURL });
  client.interceptors.request.use(cfg => {
    const t = getToken();
    if (t) cfg.headers.Authorization = `Bearer ${t}`;
    return cfg;
  });
  return client;
}