import { useEffect, useRef, useState } from "react";
import { register } from "./api/auth";
import { getRecent, sendMessage, type Message } from "./api/chat";
import { createChatConnection } from "./signalr/connection";
import type { HubConnection } from "@microsoft/signalr";

export default function App() {
  const [token, setToken] = useState<string | null>(localStorage.getItem("token"));
  const [nickname, setNickname] = useState("");
  const [text, setText] = useState("");
  const [messages, setMessages] = useState<Message[]>([]);
  const [hubState, setHubState] = useState<"disconnected" | "reconnecting" | "connected" | "error">("disconnected");
  const [sendErr, setSendErr] = useState<string | null>(null);

  const connRef = useRef<HubConnection | null>(null);
  const listRef = useRef<HTMLDivElement | null>(null);

  async function doRegister() {
    setSendErr(null);
    const nick = nickname.trim() || `guest-${Math.floor(Math.random() * 1000)}`;
    const t = await register(nick);
    localStorage.setItem("token", t);
    setToken(t);
  }

  function logout() {
    localStorage.removeItem("token");
    setToken(null);
    setMessages([]);
    setText("");
    setHubState("disconnected");
    connRef.current?.stop().catch(() => {});
    connRef.current = null;
  }

  useEffect(() => {
    const el = listRef.current;
    if (el) el.scrollTop = el.scrollHeight;
  }, [messages]);

  useEffect(() => {
    if (!token) return;

    let disposed = false;

    getRecent(token)
      .then((recent) => { if (!disposed) setMessages(recent); })
      .catch(console.error);

    const conn = createChatConnection(token);
    connRef.current = conn;

    const isActive = () => connRef.current === conn && !disposed;

    conn.on("ReceiveMessage", (msg: Message) => {
      if (!isActive()) return;
      setMessages((prev) => [...prev, msg]);
    });

    conn.onreconnecting(() => { if (isActive()) setHubState("reconnecting"); });
    conn.onreconnected(() => { if (isActive()) setHubState("connected"); });
    conn.onclose(() => { if (isActive()) setHubState("disconnected"); });

    (async () => {
      try {
        await conn.start();
        if (isActive()) setHubState("connected");
      } catch (err) {
        console.error("SignalR connection failed:", err);
        if (isActive()) setHubState("error");
      }
    })();

    return () => {
      disposed = true;
      conn.off("ReceiveMessage");
      conn.stop().catch(() => {});
      if (connRef.current === conn) connRef.current = null;
    };
  }, [token]);

  async function onSend() {
    if (!token) return;
    const t = text.trim();
    if (!t) return;

    setText("");
    setSendErr(null);
    try {
      await sendMessage(token, t); // Hub yayınlayacak
    } catch (err: any) {
      console.error(err);
      setSendErr(err?.message || "Mesaj gönderilemedi");
    }
  }

  const sendDisabled = hubState !== "connected" || !text.trim();

  function onKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      onSend();
    }
  }

  if (!token) {
    return (
      <div style={{ padding: 24, maxWidth: 480, margin: "4rem auto" }}>
        <h3>Giriş</h3>
        <div style={{ display: "flex", gap: 8 }}>
          <input
            placeholder="nickname"
            value={nickname}
            onChange={(e) => setNickname(e.target.value)}
            style={{ flex: 1, padding: 10 }}
          />
          <button onClick={doRegister}>Devam</button>
        </div>
      </div>
    );
  }

  return (
    <div style={{ padding: 24, maxWidth: 800, margin: "1rem auto", display: "flex", flexDirection: "column", height: "90vh" }}>
      <header style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 12 }}>
        <h3>AI Sohbeti</h3>
        <div style={{ display: "flex", gap: 12, alignItems: "center" }}>
          <small>Hub: {hubState}</small>
          <button onClick={logout} style={{ padding: "6px 10px" }}>Çıkış</button>
        </div>
      </header>

      <div
        ref={listRef}
        style={{
          flex: 1,
          overflowY: "auto",
          border: "1px solid #ddd",
          borderRadius: 6,
          padding: 12,
          marginBottom: 8
        }}
      >
        {messages.map((m, i) => {
          const time = m.sentAtUtc ? new Date(m.sentAtUtc).toLocaleTimeString() : "";
          return (
            <div key={(m.id ?? `${m.nickname ?? "anon"}-${m.sentAtUtc ?? ""}-${i}`).toString()} style={{ marginBottom: 10 }}>
              <div style={{ fontSize: 12, color: "#777" }}>
                <b>{m.nickname ?? "Anon"}</b> {time ? `· ${time}` : ""}
              </div>
              <div>
                {m.text}{" "}
                {("sentimentLabel" in m) && (m as any).sentimentLabel && (
                  <span style={{ marginLeft: 6, opacity: 0.8 }}>
                    ({(m as any).sentimentLabel}
                    {typeof (m as any).sentimentScore === "number" ? ` ${(m as any).sentimentScore.toFixed(2)}` : ""})
                  </span>
                )}
              </div>
            </div>
          );
        })}
      </div>

      {sendErr && <div style={{ color: "crimson", marginBottom: 8 }}>{sendErr}</div>}

      <div style={{ display: "flex", gap: 8 }}>
        <input
          value={text}
          onChange={(e) => setText(e.target.value)}
          onKeyDown={onKeyDown}
          placeholder="mesaj yaz..."
          style={{ flex: 1, padding: 12 }}
        />
        <button onClick={onSend} disabled={sendDisabled} title={hubState !== "connected" ? "Hub bağlı değil" : ""}>
          Gönder
        </button>
      </div>
    </div>
  );
}