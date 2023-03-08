import {
  Component,
  OnInit
} from '@angular/core';

import {
  IntentType,
  ProcessingTypes
} from './models';

import {
  ConsoleService,
  ErrorService,
  ProcessorService,
  SecretService
} from './services';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  intents = ProcessingTypes.intents;
  intent: IntentType = 'Acquire';

  constructor(
    private console: ConsoleService,
    public errorSvc: ErrorService,
    public processorSvc: ProcessorService,
    public secretSvc: SecretService
  ) {
    this.console.write('Azure SPA demo console initialized', 'primary');    
  }

  async ngOnInit(): Promise<void> {
    await this.processorSvc.start();
    await this.processorSvc.register();    
  }

  setIntent = (intent: IntentType) => this.intent = intent;

  submit = async () => 
    await this.processorSvc.submit(this.intent);
}
