import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AvariasComponent } from './avarias.component';

describe('AvariasComponent', () => {
  let component: AvariasComponent;
  let fixture: ComponentFixture<AvariasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AvariasComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AvariasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
