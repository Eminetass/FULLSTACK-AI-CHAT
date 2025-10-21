import * as signalR from '@microsoft/signalr';
import { CHAT_API } from '../env';
import { getToken } from '../auth';

export function createChatHubConnection() {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${CHAT_API}/hubs/chat`, {
      accessTokenFactory: () => getToken(),
    })
    .withAutomaticReconnect({
      nextRetryDelayInMilliseconds: ctx => Math.min(1000 * (ctx.previousRetryCount + 1), 10000),
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();

  return connection;
}