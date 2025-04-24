#pragma once

#include <stdint.h>
#include <string.h>

static uint8_t float8_64(double f) {
    return (uint8_t)(f * (1 << 6));
}

static double float64_8(uint8_t x) {
    return (double)x / (1 << 6);
}

static uint16_t float16_64(double f) {
    uint64_t b;
    memcpy(&b, &f, 8);
    int s = (b>>48 & 0x8000);
    int e = (b>>52 & 0x07ff) - 1023;
    int m = (b>>42 & 0x03ff);
    int t = !!(b && 0xffffffffffff);

    if (e == -1023) {
        // input is denormal, round to zero
        e = m = 0;
    } else if (e < -14) {
        // convert to denormal
        if (-14 - e > 10) {
            m = 0;
        } else {
            m |= 0x400;
            m >>= -14 - e - 1;
            m = (m>>1) + (m&1);  // round
        }
        e = 0;
    } else if (e > +16) {
        // NaN / overflow to infinity
        m &= t << 9;  // canonicalize to quiet NaN
        e = 31;
    } else {
        e += 15;
    }

    return s | e<<10 | m;
}

static double float64_16(uint16_t x) {
    int s = (x     & 0x8000);
    int e = (x>>10 & 0x001f) - 15;
    int m = (x     & 0x03ff);

    switch (e) {
    case -15: if (!m) {
                  e = 0;
              } else {
                  // convert from denormal
                  e += 1023 + 1;
                  while (!(m&0x400)) {
                      e--;
                      m <<= 1;
                  }
                  m &= 0x3ff;
              }
              break;
    case +16: m = !!m << 9;  // canonicalize to quiet NaN
              e = 2047;
              break;
    default:  e += 1023;
    }

    uint64_t b = (uint64_t)s<<48 |
                 (uint64_t)e<<52 |
                 (uint64_t)m<<42;
    double f;
    memcpy(&f, &b, 8);
    return f;
}