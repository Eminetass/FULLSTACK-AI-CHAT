import axios from "axios";
const CHAT_API = import.meta.env.VITE_CHAT_API as string;

export type Message = {
  id: string;
  userId: string;
  nickname: string;
  text: string;
  sentAtUtc: string;
  sentimentLabel?: string | null;
  sentimentScore?: number | null;
};

export async function sendMessage(token: string, text: string): Promise<Message> {
  const { data } = await axios.post(`${CHAT_API}/api/messages`, { text }, {
    headers: { Authorization: `Bearer ${token}` },
  });
  return data as Message;
}

export async function getRecent(token: string, take = 50): Promise<Message[]> {
  const { data } = await axios.get(`${CHAT_API}/api/messages/recent?take=${take}`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  return data as Message[];
}