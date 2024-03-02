import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ObfuscationComponent } from './obfuscation.component';

describe('ObfuscationComponent', () => {
  let component: ObfuscationComponent;
  let fixture: ComponentFixture<ObfuscationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ObfuscationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ObfuscationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
