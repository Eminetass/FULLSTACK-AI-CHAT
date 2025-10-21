import * as signalR from "@microsoft/signalr";

export function createChatConnection(token: string) {
  return new signalR.HubConnectionBuilder()
    .withUrl(`${import.meta.env.VITE_CHAT_API}/hubs/chat`, {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .build();
}