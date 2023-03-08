import { Injectable } from '@angular/core';

import {
    ConsoleColor,
    ConsoleFormats,
    ConsoleMessage,
    ErrorResult
} from '../models';

@Injectable({
    providedIn: 'root'
})
export class ConsoleService {
    colors = ConsoleFormats.colors;
    messages: ConsoleMessage[] = new Array<ConsoleMessage>();

    write(message: string, color: ConsoleColor = 'text') {
        this.messages.push({ message, color } as ConsoleMessage);
    }

    error(result: ErrorResult) {
        this.messages.push({ message: JSON.stringify(result, null, 2), color: 'danger' } as ConsoleMessage);
    }
}