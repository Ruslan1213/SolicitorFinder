import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import SolicitorCard from '../SolicitorCard.vue'

describe('SolicitorCard Snapshots', () => {
  it('matches snapshot with full solicitor data', () => {
    const mockSolicitor = {
      name: 'Test Solicitor & Associates',
      ratingStars: 4.7,
      reviewCount: 150,
      location: 'London, UK',
      phone: '+44 20 1234 5678',
      emailLink: 'mailto:info@test.com',
      website: 'https://example.com',
      address: '123 Test Street, London, SW1A 1AA',
      description: 'A professional solicitor firm specializing in family law and commercial disputes.',
      scrapedAt: '2024-06-27T10:30:00Z'
    }

    const wrapper = mount(SolicitorCard, {
      props: { solicitor: mockSolicitor }
    })

    expect(wrapper.html()).toMatchSnapshot()
  })

  it('matches snapshot with minimal solicitor data', () => {
    const mockSolicitor = {
      name: 'Minimal Solicitor',
      ratingStars: 3.0,
      reviewCount: 5,
      scrapedAt: '2024-06-27T10:30:00Z'
    }

    const wrapper = mount(SolicitorCard, {
      props: { solicitor: mockSolicitor }
    })

    expect(wrapper.html()).toMatchSnapshot()
  })

  it('matches snapshot with perfect 5-star rating', () => {
    const mockSolicitor = {
      name: 'Perfect Solicitor',
      ratingStars: 5.0,
      reviewCount: 500,
      location: 'Manchester',
      scrapedAt: '2024-06-27T10:30:00Z'
    }

    const wrapper = mount(SolicitorCard, {
      props: { solicitor: mockSolicitor }
    })

    expect(wrapper.html()).toMatchSnapshot()
  })
})
