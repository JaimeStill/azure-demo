import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { ConsoleService } from './console.service';

import {
    ErrorResult,
    SecretResult
} from '../models';

@Injectable({
    providedIn: 'root'
})
export class SecretService {
    constructor(
        private console: ConsoleService,
        private http: HttpClient
    ) { }

    get = () => this.http.get<SecretResult>(`${environment.api}secret`)
        .subscribe({
            next: (result: SecretResult) => {
                this.console.write(`${result.name}: ${result.value}`);
                this.console.write(result.message, 'warning');
            },
            error: (err: any) =>
                this.console.error(err.error as ErrorResult)
        })
        
}