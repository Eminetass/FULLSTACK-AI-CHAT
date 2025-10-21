import { useState } from 'react';
import { register } from '../lib/api/auth';

export default function Login({ onLoggedIn }: { onLoggedIn: () => void }) {
  const [nickname, setNickname] = useState('');
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<string | null>(null);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setErr(null); setLoading(true);
    try {
      await register(nickname.trim());
      onLoggedIn();
    } catch (e: any) {
      setErr(e?.message || 'Kayıt başarısız');
    } finally {
      setLoading(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} style={{ maxWidth: 420, margin: '4rem auto' }}>
      <h2>Rumuzunla giriş yap</h2>
      <input
        value={nickname}
        onChange={e => setNickname(e.target.value)}
        placeholder="Rumuz"
        required
        style={{ width: '100%', padding: 12, marginTop: 12 }}
      />
      <button disabled={loading} style={{ marginTop: 12, padding: 12, width: '100%' }}>
        {loading ? 'Gönderiliyor...' : 'Devam'}
      </button>
      {err && <p style={{ color: 'red' }}>{err}</p>}
    </form>
  );
}