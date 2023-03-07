import { Intent } from './intent';
import { Resource } from './resource';

export interface Package {
    key: string;
    name: string;
    intent: Intent;

    resources: Resource[];
}