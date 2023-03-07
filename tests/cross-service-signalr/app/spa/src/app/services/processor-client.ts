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
        this.onPush = this.output;
        this.onNotify = this.output;
        this.onComplete = this.output;
        this.onReturn = this.output;
        this.onReject = this.output;
    }

    constructor() {
        super('http://localhost:5100/processor');
        this.initEvents();
    }

    output = (message: SyncMessage<Package>) => {
        console.log(`${SyncActionType[message.action]}: {message.message}`);
        this.messages.push(`${SyncActionType[message.action]}: {message.message}`);
    }
}