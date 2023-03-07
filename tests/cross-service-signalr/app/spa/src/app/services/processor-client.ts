import { Injectable } from '@angular/core';
import { SyncClient } from './sync-client';

import {
    Package,
    SyncActionType,
    SyncMessage
} from '../models';

@Injectable({
    providedIn: 'root'
})
export class ProcessorClient extends SyncClient<Package> {
    private initEvents = () => {
        this.onPush.set(this.output);
        this.onNotify.set(this.output);
        this.onComplete.set(this.output);
        this.onReturn.set(this.output);
        this.onReject.set(this.output);
    }

    constructor() {
        super('http://localhost:5100/processor');
        this.initEvents();
    }

    output = (message: SyncMessage<Package>) => {
        console.log(`${SyncActionType[message.action]}: ${message.message}`);
        this.messages.push(`${SyncActionType[message.action]}: ${message.message}`);
    }
}