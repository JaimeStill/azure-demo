import {
    HubConnection,
    HubConnectionBuilder,
    LogLevel
} from '@microsoft/signalr';

import { SyncAction } from '../models';
import { ConsoleService } from './console.service';
import { v4 as uuid } from 'uuid';

export abstract class SyncService {
    protected key: string;
    protected client: HubConnection;
    protected console: ConsoleService;

    connected: boolean = false;
    endpoint: string;

    onRegistered: SyncAction;
    onPush: SyncAction;
    onNotify: SyncAction;
    onComplete: SyncAction;
    onReturn: SyncAction;
    onReject: SyncAction;

    protected initializeEvents = () => {
        this.client.onclose(async () => {
            this.connected = false;
            await this.start();
        });

        this.onRegistered = new SyncAction("Registered", this.client);
        this.onPush = new SyncAction("Push", this.client);
        this.onNotify = new SyncAction("Notify", this.client);
        this.onComplete = new SyncAction("Complete", this.client);
        this.onReturn = new SyncAction("Return", this.client);
        this.onReject = new SyncAction("Reject", this.client);

        this.onRegistered.set((guid: string) => {
            this.key = guid
        });
    }

    protected initialize = () => {
        this.client = new HubConnectionBuilder()
            .withUrl(this.endpoint, {
                withCredentials: true
            })
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        this.initializeEvents();
    }

    constructor(endpoint: string, console: ConsoleService) {
        this.endpoint = endpoint;
        this.console = console;
        this.initialize();
    }

    state = () => this.client.state;

    start = async () => {
        try {
            this.console.write(`Sync Client: Connecting to ${this.endpoint}`);
            await this.client.start();
            this.console.write(`Sync Client: Connected to ${this.endpoint}`);
            this.connected = true;
        } catch (err) {
            this.console.error(err.error);
            setTimeout(this.start, 5000);
        }
    }

    register = async () => await this.client.invoke("registerListener", uuid());
}
