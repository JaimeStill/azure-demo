import { SyncActionType } from './sync-action-type';

export interface SyncMessage<T> {
    id: string;
    key: string;
    message: string;
    data: T;
    action: SyncActionType;
}