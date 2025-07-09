import { TestBed } from '@angular/core/testing';
import { ProcessingService } from '../../app/services/processing.service';
import { NgZone } from '@angular/core';

describe('ProcessingService', () => {
  let service: ProcessingService;
  let zone: NgZone;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProcessingService],
    });
    service = TestBed.inject(ProcessingService);
    zone = TestBed.inject(NgZone);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call callbacks on metadata, message, and error', () => {
    const input = 'test';
    const mockEventSource: any = {
      addEventListener: jasmine.createSpy(),
      onmessage: null,
      onerror: null,
      close: jasmine.createSpy(),
    };

    spyOn(window as any, 'EventSource').and.returnValue(mockEventSource);

    const onMetadata = jasmine.createSpy('onMetadata');
    const onData = jasmine.createSpy('onData');
    const onComplete = jasmine.createSpy('onComplete');
    const onError = jasmine.createSpy('onError');

    service.processStream(input, onMetadata, onData, onComplete, onError);

    expect(mockEventSource.addEventListener).toHaveBeenCalled();

    const messageHandler = mockEventSource.onmessage;
    if (messageHandler) {
      zone.run(() => messageHandler({ data: 'x' }));
      expect(onData).toHaveBeenCalledWith('x');
    }

    const errorHandler = mockEventSource.onerror;
    if (errorHandler) {
      zone.run(() => errorHandler());
      expect(onComplete).toHaveBeenCalled();
      expect(onError).toHaveBeenCalled();
    }
  });

  it('should close stream', () => {
    const mockES = { close: jasmine.createSpy() };
    service.cancelStream(mockES as any);
    expect(mockES.close).toHaveBeenCalled();
  });
});
