import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaineisSolaresComponent } from './paineis-solares.component';

describe('PaineisSolaresComponent', () => {
  let component: PaineisSolaresComponent;
  let fixture: ComponentFixture<PaineisSolaresComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PaineisSolaresComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaineisSolaresComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
