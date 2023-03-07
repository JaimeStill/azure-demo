import { HubConnection } from '@microsoft/signalr';

export class SyncAction {
    constructor(
        private action: string,
        private client: HubConnection
    ) { }

    set = (method: (...args: any[]) => any) => {
        this.client.on(this.action, method);
    }
}