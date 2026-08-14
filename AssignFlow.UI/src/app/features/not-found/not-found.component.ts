import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink, ButtonModule],
  template: `
    <main class="not-found">
      <div>
        <span>404</span>
        <h1>This page stepped out.</h1>
        <p>The address may have changed or you may not have access to this workspace.</p>
        <p-button label="Return to overview" icon="pi pi-arrow-left" routerLink="/dashboard" />
      </div>
    </main>
  `,
  styles: [`
    .not-found { display: grid; min-height: 100vh; place-items: center; padding: 32px; text-align: center; background: radial-gradient(circle at 50% 25%, #e1f2f2, #f6fafb 48%); }
    .not-found div { max-width: 520px; }
    span { display: block; color: #2f888c; font-size: 13px; font-weight: 900; letter-spacing: .22em; }
    h1 { margin: 18px 0 10px; color: #123542; font-size: clamp(34px, 6vw, 58px); letter-spacing: -.05em; }
    p { margin: 0 auto 28px; color: #708790; line-height: 1.6; }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NotFoundComponent {}
