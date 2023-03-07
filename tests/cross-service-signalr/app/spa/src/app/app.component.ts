import {
  Component,
  OnInit
} from '@angular/core';

import { HubConnectionState } from '@microsoft/signalr';
import { ProcessorClient } from './services';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html'
})
export class AppComponent implements OnInit {
  constructor(
    public client: ProcessorClient
  ) { }
  async ngOnInit(): Promise<void> {
    await this.client.start();
    await this.client.register();
  }

  connected = () => this.client.state() === HubConnectionState.Connected;
}
