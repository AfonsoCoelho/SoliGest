import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PwrecoveryComponent } from './pwrecovery.component';

describe('PwrecoveryComponent', () => {
  let component: PwrecoveryComponent;
  let fixture: ComponentFixture<PwrecoveryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PwrecoveryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PwrecoveryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
