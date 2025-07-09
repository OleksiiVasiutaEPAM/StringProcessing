import { Component, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ProcessingService } from './services/processing.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
})
export class AppComponent {
  inputText = '';
  output = '';
  progress = 0;
  processing = false;
  private expectedLength: number | null = null;
  private eventSource?: EventSource;

  constructor(
    private processingService: ProcessingService,
    private cdr: ChangeDetectorRef
  ) {}

  start() {
    if (!this.inputText.trim() || this.processing)
      return;

    this.output = '';
    this.progress = 0;
    this.expectedLength = null;
    this.processing = true;

    this.eventSource = this.processingService.processStream(
      this.inputText,
      (length) => {
        this.expectedLength = length;
        this.cdr.markForCheck();
      },
      (chunk) => {
        this.output += chunk;
        if (this.expectedLength) {
          this.progress = Math.min((this.output.length / this.expectedLength) * 100, 100);
          if (this.output.length >= this.expectedLength) {
            this.finishProcessing();
          }
        }
        this.cdr.markForCheck();
      },
      () => this.finishProcessing(),
      () => this.finishProcessing()
    );
  }

  cancel() {
    this.processingService.cancelStream(this.eventSource);
    this.processing = false;
  }

  private finishProcessing() {
    this.processingService.cancelStream(this.eventSource);
    this.processing = false;
    this.expectedLength = null;
    this.progress = 0;
  }
}