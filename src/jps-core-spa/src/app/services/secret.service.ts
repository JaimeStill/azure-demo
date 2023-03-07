import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { SecretResult } from '../models';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class SecretService {
    constructor(
        private http: HttpClient
    ) { }

    get = (): Promise<SecretResult> =>
        firstValueFrom(
            this.http.get<SecretResult>(`${environment.api}secret`)
        );
}