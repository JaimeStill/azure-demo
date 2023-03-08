import {
    AfterViewChecked,
    Component,
    ElementRef,
    Input,
    ViewChild
} from '@angular/core';

import { ConsoleService } from '../services';

@Component({
    selector: 'console',
    templateUrl: 'console.component.html',
    styleUrls: ['console.component.scss']
})
export class ConsoleComponent implements AfterViewChecked {
    @Input() height: number = 360;
    
    @ViewChild('console') console: ElementRef;

    constructor(
        public consoleSvc: ConsoleService
    ) { }

    private scrollConsole = () => {
        if (this.console?.nativeElement) {
            this.console.nativeElement.scrollTop = this.console.nativeElement.scrollHeight + 100;
        }
    }

    ngAfterViewChecked(): void {
        this.scrollConsole();
    }
}