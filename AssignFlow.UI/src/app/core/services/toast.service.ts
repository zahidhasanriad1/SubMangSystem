import { inject, Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly messages = inject(MessageService);

  success(detail: string): void {
    this.show('success', 'Success', detail);
  }

  error(detail: string): void {
    this.show('error', 'Request failed', detail, 4500);
  }

  info(detail: string): void {
    this.show('info', 'Information', detail);
  }

  warn(detail: string): void {
    this.show('warn', 'Attention', detail);
  }

  private show(severity: string, summary: string, detail: string, life = 3000): void {
    this.messages.add({ severity, summary, detail, life });
  }
}
