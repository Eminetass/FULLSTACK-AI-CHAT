export const IDENTITY_API = import.meta.env.VITE_IDENTITY_API as string;
export const CHAT_API = import.meta.env.VITE_CHAT_API as string;

if (!IDENTITY_API || !CHAT_API) {
  console.warn('VITE_IDENTITY_API veya VITE_CHAT_API tanımlı değil.');
}