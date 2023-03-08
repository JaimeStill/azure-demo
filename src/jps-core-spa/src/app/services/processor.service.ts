import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SyncService } from './sync.service';
import { environment } from '../../environments/environment';
import { ConsoleService } from './console.service';
import { v4 as uuid } from 'uuid';

import {
    ConsoleColor,
    Intent,
    IntentType,
    Package,
    Resource,
    SyncActionType,
    SyncMessage
} from '../models';

@Injectable({
    providedIn: 'root'
})
export class ProcessorService extends SyncService {
    private initEvents = () => {
        this.onPush.set(this.output('info'));
        this.onNotify.set(this.output());
        this.onComplete.set(this.output('success'));
        this.onReturn.set(this.output('warning'));
        this.onReject.set(this.output('danger'));
    }

    constructor(
        private http: HttpClient,
        console: ConsoleService
    ) {
        super(`${environment.sync}processor`, console);
        this.initEvents();
    }

    submit = (intent: IntentType): Promise<boolean> =>
        new Promise((resolve, reject) => {
            const pkg: Package = this.generatePackage(intent);
            this.http.post<boolean>(`${environment.api}process`, pkg)
                .subscribe({
                    next: (result: boolean) => resolve(result),
                    error: (err: any) => {
                        this.console.error(err.error);
                        reject(err);
                    }
                })
        });

    output = (color: ConsoleColor = 'text') =>
        (message: SyncMessage<Package>) => 
            this.console.write(`Sync ${SyncActionType[message.action]}: ${message.message}`, color);

    private generatePackage = (intent: IntentType): Package => {
        switch (intent) {
            case 'Acquire':
                return this.acquisition();
            case 'Approve':
                return this.approval();
            case 'Destroy':
                return this.destruction();
            case 'Transfer':
                return this.transfer();
        }
    }

    private acquisition = () => <Package>{
        key: uuid(),
        name: 'SPA Acquisition Package',
        intent: Intent.Acquire,
        resources: this.generateResources()
    }

    private approval = () => <Package>{
        key: uuid(),
        name: 'SPA Approval Package',
        intent: Intent.Approve,
        resources: this.generateResources()
    }
    
    private destruction = () => <Package>{
        key: uuid(),
        name: 'SPA Destruction Package',
        intent: Intent.Destroy,
        resources: this.generateResources()
    }

    private transfer = () => <Package>{
        key: uuid(),
        name: 'SPA Transfer Package',
        intent: Intent.Transfer,
        resources: this.generateResources()
    }

    private generateResources = () => <Resource[]>[
        <Resource>{ key: uuid(), name: 'Training Plan' },
        <Resource>{ key: uuid(), name: 'R&D Proposal' }
    ]
}