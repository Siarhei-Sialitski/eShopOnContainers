import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { OrdersService } from '../orders.service';
import { BasketService } from '../../basket/basket.service';
import { IOrder }                                   from '../../shared/models/order.model';
import { BasketWrapperService }                     from '../../shared/services/basket.wrapper.service';
import { ICoupon } from '../../shared/models/coupon.model';
import { FormGroup, FormBuilder, Validators  }      from '@angular/forms';
import { Router }                                   from '@angular/router';
import { ILoyaltyMember } from "../../shared/models/loyaltyMember.model";

@Component({
  selector: "esh-orders_new .esh-orders_new .mb-5",
  styleUrls: ["./orders-new.component.scss"],
  templateUrl: "./orders-new.component.html",
})
export class OrdersNewComponent implements OnInit {
  newOrderForm: FormGroup; // new order form
  isOrderProcessing: boolean;
  errorReceived: boolean;
  coupon: ICoupon;
  order: IOrder;
  couponValidationMessage: string;
  loyaltyMember: ILoyaltyMember;
  loyaltyMemberMessage: string;
  pointsValidationMessage: string;
  membershipDiscountPercent: number;
  membershipDiscount: number;

  constructor(
    private orderService: OrdersService,
    private basketService: BasketService,
    fb: FormBuilder,
    private router: Router
  ) {
    // Obtain user profile information
    this.order = orderService.mapOrderAndIdentityInfoNewOrder();
    this.newOrderForm = fb.group({
      street: [this.order.street, Validators.required],
      city: [this.order.city, Validators.required],
      state: [this.order.state, Validators.required],
      country: [this.order.country, Validators.required],
      cardnumber: [this.order.cardnumber, Validators.required],
      cardholdername: [this.order.cardholdername, Validators.required],
      expirationdate: [this.order.expiration, Validators.required],
      securitycode: [this.order.cardsecuritynumber, Validators.required],
    });
  }

  ngOnInit() {
    this.orderService.getLoyaltyMemberInfo().subscribe(
      (loyaltyMember) => {
        this.loyaltyMember = loyaltyMember;
        this.membershipDiscountPercent =
          loyaltyMember.transactions >= 20
            ? 0.2
            : loyaltyMember.transactions >= 10
            ? 0.1
            : 0.05;
        this.membershipDiscount =
          this.order.total * this.membershipDiscountPercent;
        this.order.total = this.order.total - this.membershipDiscount;
      },
      (error) =>
        (this.loyaltyMemberMessage = "The loyalty member is not valid!")
    );
  }

  keyDownValidationCoupon(event: KeyboardEvent, discountCode: string) {
    if (event.keyCode === 13) {
      event.preventDefault();
      this.checkValidationCoupon(discountCode);
    }
  }

  checkValidationPoints(points: number) {
    if (points <= this.loyaltyMember.points) {
      this.loyaltyMemberMessage = null;
      this.order.total = this.order.total - points;
      this.loyaltyMember.points = this.loyaltyMember.points - points;
      this.orderService.updateLoyaltyMemberInfo(this.loyaltyMember).subscribe(
        (loyaltyMember) => (this.loyaltyMember = loyaltyMember),
        (error) =>
          (this.loyaltyMemberMessage = "The loyalty member is not valid!")
      );
      console.log(`${points} points used.`);
    } else {
      this.pointsValidationMessage = "You don't have enough points to use!";
    }
  }

  checkValidationCoupon(discountCode: string) {
    this.couponValidationMessage = null;
    this.coupon = null;
    this.orderService.checkValidationCoupon(discountCode).subscribe(
      (coupon) => (this.coupon = coupon),
      (error) =>
        (this.couponValidationMessage =
          "The coupon is not valid or it's been redeemed already!")
    );
  }

  submitForm(value: any) {
    this.order.street = this.newOrderForm.controls["street"].value;
    this.order.city = this.newOrderForm.controls["city"].value;
    this.order.state = this.newOrderForm.controls["state"].value;
    this.order.country = this.newOrderForm.controls["country"].value;
    this.order.cardnumber = this.newOrderForm.controls["cardnumber"].value;
    this.order.cardtypeid = 1;
    this.order.cardholdername =
      this.newOrderForm.controls["cardholdername"].value;
    this.order.cardexpiration = new Date(
      20 + this.newOrderForm.controls["expirationdate"].value.split("/")[1],
      this.newOrderForm.controls["expirationdate"].value.split("/")[0]
    );
    this.order.cardsecuritynumber =
      this.newOrderForm.controls["securitycode"].value;

    if (this.coupon) {
      console.log(`Coupon: ${this.coupon.code} (${this.coupon.discount})`);

      this.order.coupon = this.coupon.code;
      this.order.discount = this.coupon.discount;
    }

    let basketCheckout = this.basketService.mapBasketInfoCheckout(this.order);
    this.basketService
      .setBasketCheckout(basketCheckout)
      .pipe(
        catchError((errMessage) => {
          this.errorReceived = true;
          this.isOrderProcessing = false;
          return Observable.throw(errMessage);
        })
      )
      .subscribe((res) => {
        this.router.navigate(["orders"]);
      });
    this.errorReceived = false;
    this.isOrderProcessing = true;
  }
}

