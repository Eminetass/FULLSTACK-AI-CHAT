import { useEffect, useMemo, useRef, useState } from 'react';
import { getRecent, sendMessage } from '../lib/api/chat';
import { createChatHubConnection } from '../lib/signalr/connection';

type Sentiment = 'positive' | 'negative' | 'neutral' | null;
type Message = {
  id?: string;
  text: string;
  nickname?: string;
  sentiment?: Sentiment;
  sentAtUtc?: string;
};

export default function Chat() {
  const [messages, setMessages] = useState<Message[]>([]);
  const [text, setText] = useState('');
  const [loading, setLoading] = useState(true);
  const [hubState, setHubState] = useState('disconnected');

  const connection = useMemo(() => createChatHubConnection(), []);
  const listRef = useRef<HTMLDivElement>(null);

  function scrollBottom() {
    listRef.current?.scrollTo({ top: listRef.current.scrollHeight, behavior: 'smooth' });
  }

  useEffect(() => {
    let mounted = true;

    // Hub event adı backende göre değişebilir: "ReceiveMessage" / "messageAdded"
    connection.on('ReceiveMessage', (msg: Message) => {
      if (!mounted) return;
      setMessages(prev => [...prev, msg]);
      setTimeout(scrollBottom, 0);
    });

    connection.onreconnected(() => setHubState('connected'));
    connection.onclose(() => setHubState('disconnected'));
    connection.onreconnecting(() => setHubState('reconnecting'));

    (async () => {
      try {
        const recent = await getRecent(50);
        if (mounted) setMessages(recent || []);
      } finally {
        setLoading(false);
      }

      try {
        await connection.start();
        setHubState('connected');
      } catch (err) {
        console.error('SignalR bağlanamadı:', err);
        setHubState('error');
      }
    })();

    return () => {
      mounted = false;
      connection.stop().catch(() => {});
      connection.off('ReceiveMessage');
    };
  }, [connection]);

  async function handleSend(e: React.FormEvent) {
    e.preventDefault();
    const t = text.trim();
    if (!t) return;
    setText('');
    try {
      await sendMessage(t);
      // Optimistic UI istersen burada mesajı geçici olarak ekleyebilirsin.
      // Hub yayını gelince gerçek mesaj listeye eklenecek.
    } catch (err) {
      console.error(err);
      // Hata uyarısı göster
    }
  }

  return (
    <div style={{ maxWidth: 720, margin: '1rem auto', display: 'flex', flexDirection: 'column', height: '90vh' }}>
      <header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h2>Chat</h2>
        <small>Hub: {hubState}</small>
      </header>

      <div ref={listRef} style={{ flex: 1, overflowY: 'auto', padding: '1rem', border: '1px solid #eee' }}>
        {loading && <p>Yükleniyor...</p>}
        {!loading && messages.length === 0 && <p>Henüz mesaj yok.</p>}
        {messages.map((m, i) => (
          <div key={m.id ?? i} style={{ marginBottom: 8 }}>
            <div style={{ fontSize: 12, color: '#777' }}>
              {m.nickname ?? 'Anon'} · {new Date(m.sentAtUtc ?? Date.now()).toLocaleTimeString()}
            </div>
            <div>
              {m.text}{' '}
              {m.sentiment && (
                <span style={{
                  marginLeft: 8,
                  padding: '2px 6px',
                  borderRadius: 8,
                  background: m.sentiment === 'positive' ? '#def8de' :
                              m.sentiment === 'negative' ? '#fde2e2' : '#eef1f6',
                  color: '#333',
                  fontSize: 12
                }}>
                  {m.sentiment}
                </span>
              )}
            </div>
          </div>
        ))}
      </div>

      <form onSubmit={handleSend} style={{ display: 'flex', gap: 8, marginTop: 8 }}>
        <input
          value={text}
          onChange={e => setText(e.target.value)}
          placeholder="Mesaj yaz..."
          style={{ flex: 1, padding: 12 }}
        />
        <button type="submit">Gönder</button>
      </form>
    </div>
  );
}