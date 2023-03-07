import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { ErrorResult } from '../models';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ErrorService {
    private error = new BehaviorSubject<ErrorResult>(null);
    error$ = this.error.asObservable();

    constructor(
        private http: HttpClient
    ) { }

    get = () => this.http.get(`${environment.api}error`)
        .subscribe({
            error: (err: ErrorResult) => this.error.next(err)
        });
}