import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ErrorResult } from '../models';
import { environment } from '../../environments/environment';
import { ConsoleService } from './console.service';

@Injectable({
    providedIn: 'root'
})
export class ErrorService {
    constructor(
        private console: ConsoleService,
        private http: HttpClient
    ) { }

    get = () => this.http.get(`${environment.api}error`)
        .subscribe({
            error: (err: any) =>
                this.console.error(err.error as ErrorResult)
        });
}