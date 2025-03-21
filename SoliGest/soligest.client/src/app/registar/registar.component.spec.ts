import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegistarComponent } from './registar.component';

describe('RegistarComponent', () => {
  let component: RegistarComponent;
  let fixture: ComponentFixture<RegistarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RegistarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegistarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
