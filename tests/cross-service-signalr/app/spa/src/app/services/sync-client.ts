import {
    HubConnection,
    HubConnectionBuilder,
    LogLevel
} from '@microsoft/signalr';

import {
    SyncAction,
    SyncMessage
} from '../models';

import { v4 as uuid } from 'uuid';

export abstract class SyncClient<T> {
    protected key: string;
    protected client: HubConnection;

    connected: boolean = false;
    endpoint: string;
    messages: string[] = new Array<string>();

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
            this.key = guid;
        });
    }

    protected initialize = () => {
        this.client = new HubConnectionBuilder()
            .withUrl(this.endpoint)
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        this.initializeEvents();
    }

    constructor(endpoint: string) {
        this.endpoint = endpoint;
        this.initialize();
    }

    state = () => this.client.state;

    start = async () => {
        try {
            this.messages.push(`Connecting to ${this.endpoint}`);
            await this.client.start();
            this.messages.push(`Connected to ${this.endpoint}`);
            this.connected = true;
        } catch (err) {
            console.log(err);
            setTimeout(this.start, 5000);
        }
    }

    register = async () => await this.client.invoke("registerListener", uuid())
}