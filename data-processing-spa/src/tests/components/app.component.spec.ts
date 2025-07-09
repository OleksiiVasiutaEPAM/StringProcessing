import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AppComponent } from '../../app/app.component';
import { ProcessingService } from '../../app/services/processing.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

describe('AppComponent', () => {
  let fixture: ComponentFixture<AppComponent>;
  let component: AppComponent;
  let mockService: jasmine.SpyObj<ProcessingService>;

  beforeEach(async () => {
    mockService = jasmine.createSpyObj('ProcessingService', ['processStream', 'cancelStream']);

    await TestBed.configureTestingModule({
      imports: [AppComponent, FormsModule, CommonModule],
      providers: [{ provide: ProcessingService, useValue: mockService }],
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
  });

  it('should start processing and update output and progress', fakeAsync(() => {
    const fakeEventSource = {} as EventSource;

    let metadataCb: (len: number) => void = () => {};
    let dataCb: (chunk: string) => void = () => {};
    let completeCb: () => void = () => {};
    let errorCb: () => void = () => {};

    mockService.processStream.and.callFake((_input, onMeta, onData, onComplete, onError) => {
      metadataCb = onMeta;
      dataCb = onData;
      completeCb = onComplete;
      errorCb = onError;
      return fakeEventSource;
    });

    component.inputText = 'test';
    component.start();

    metadataCb(4);

    dataCb('t');
    dataCb('e');
    dataCb('s');
    dataCb('t');

    tick();

    expect(component.output).toBe('test');
    expect(component.processing).toBeFalse();
  }));
});