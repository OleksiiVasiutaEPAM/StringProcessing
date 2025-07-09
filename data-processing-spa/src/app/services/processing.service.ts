import { Injectable, NgZone } from '@angular/core';

import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ProcessingService {
  constructor(private ngZone: NgZone) {}

  processStream(
    input: string,
    onMetadata: (length: number) => void,
    onData: (chunk: string) => void,
    onComplete: () => void,
    onError: () => void
  ): EventSource {
    const es = new EventSource(`${environment.apiBaseUrl}/processing/stream?input=${encodeURIComponent(input)}`);

    es.addEventListener('metadata', (e: MessageEvent) => {
      this.ngZone.run(() => {
        const total = parseInt(e.data, 10);
        if (!isNaN(total)) {
          onMetadata(total);
        }
      });
    });

    es.onmessage = (e) => {
      this.ngZone.run(() => {
        onData(e.data);
      });
    };

    es.onerror = () => {
      this.ngZone.run(() => {
        es.close();
        onError();
        onComplete();
      });
    };

    return es;
  }

  cancelStream(es?: EventSource) {
    es?.close();
  }
}
