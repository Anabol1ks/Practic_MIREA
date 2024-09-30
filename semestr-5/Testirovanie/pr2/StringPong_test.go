package main

import "testing"

func TestStr_OnlySim(t *testing.T) {
	tests := []struct {
		name     string
		s        Str
		expected string
	}{
		{
			name:     "1",
			s:        "abc123",
			expected: "abc",
		},
		{
			name:     "2",
			s:        "12345",
			expected: "",
		},
		{
			name:     "3",
			s:        "abcdef",
			expected: "abcdef",
		},
		{
			name:     "4",
			s:        "a1!b2@c3#",
			expected: "a!b@c#",
		},
		{
			name:     "5",
			s:        "",
			expected: "",
		},
		{
			name:     "6",
			s:        "123!@#",
			expected: "!@#",
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			result := tt.s.OnlySim()
			if result != tt.expected {
				t.Errorf("OnlySim() = %v, want %v", result, tt.expected)
			}
		})
	}
}
