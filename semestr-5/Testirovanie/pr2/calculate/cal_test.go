package main

import (
	"testing"
)

func TestAdd(t *testing.T) {
	type args struct {
		a float64
		b float64
	}
	tests := []struct {
		name string
		args args
		want float64
	}{
		{
			"Test1",
			args{12, 7},
			19,
		},
		{
			"Test2",
			args{-2, 5},
			3,
		},
		{
			"Test3",
			args{99, -7},
			92,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := Add(tt.args.a, tt.args.b); got != tt.want {
				t.Errorf("Add() = %v, want %v", got, tt.want)
			}
		})
	}
}

func TestSubtract(t *testing.T) {
	type args struct {
		a float64
		b float64
	}
	tests := []struct {
		name string
		args args
		want float64
	}{
		{
			"Test1",
			args{12, 7},
			5,
		},
		{
			"Test2",
			args{-2, 5},
			-7,
		},
		{
			"Test3",
			args{99, -7},
			106,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := Subtract(tt.args.a, tt.args.b); got != tt.want {
				t.Errorf("Subtract() = %v, want %v", got, tt.want)
			}
		})
	}
}

func TestMultiply(t *testing.T) {
	type args struct {
		a float64
		b float64
	}
	tests := []struct {
		name string
		args args
		want float64
	}{
		{
			"Test1",
			args{12, 5},
			60,
		},
		{
			"Test2",
			args{-2, 5},
			-10,
		},
		{
			"Test3",
			args{123123, 0},
			0,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := Multiply(tt.args.a, tt.args.b); got != tt.want {
				t.Errorf("Multiply() = %v, want %v", got, tt.want)
			}
		})
	}
}

func TestDivide(t *testing.T) {
	type args struct {
		a float64
		b float64
	}
	tests := []struct {
		name    string
		args    args
		want    float64
		wantErr bool
	}{
		{
			"Test1",
			args{12, 3},
			4,
			false,
		},
		{
			"Test2",
			args{12, 0},
			0,
			true,
		},
		{
			"Test3",
			args{5, 2},
			2.5,
			false,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			got, err := Divide(tt.args.a, tt.args.b)
			if (err != nil) != tt.wantErr {
				t.Errorf("Divide() error = %v, wantErr %v", err, tt.wantErr)
				return
			}
			if got != tt.want {
				t.Errorf("Divide() = %v, want %v", got, tt.want)
			}
		})
	}
}

func TestMod(t *testing.T) {
	type args struct {
		a float64
	}
	tests := []struct {
		name string
		args args
		want float64
	}{
		{
			"Test1",
			args{12},
			12,
		},
		{
			"Test2",
			args{-12},
			12,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := Mod(tt.args.a); got != tt.want {
				t.Errorf("Mod() = %v, want %v", got, tt.want)
			}
		})
	}
}
