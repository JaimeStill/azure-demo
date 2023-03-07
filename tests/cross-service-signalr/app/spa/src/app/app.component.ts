import {
  Component,
  OnInit
} from '@angular/core';

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
}
