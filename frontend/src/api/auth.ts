import axios from "axios";
const ID_API = import.meta.env.VITE_IDENTITY_API as string;

export async function register(nickname: string): Promise<string> {
  const { data } = await axios.post(`${ID_API}/api/auth/register`, { nickname });
  return data.token as string;
}