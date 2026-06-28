import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import SolicitorCard from '../SolicitorCard.vue'

describe('SolicitorCard', () => {
  const mockSolicitor = {
    name: 'Test Solicitor',
    ratingStars: 4.5,
    reviewCount: 120,
    location: 'London, UK',
    phone: '+44 20 1234 5678',
    website: 'https://example.com',
    address: '123 Test Street, London',
    description: 'A professional solicitor firm',
    scrapedAt: '2024-01-15T10:30:00Z'
  }

  it('renders solicitor name correctly', () => {
    const wrapper = mount(SolicitorCard, {
      props: { solicitor: mockSolicitor }
    })

    expect(wrapper.find('.name').text()).toBe('Test Solicitor')
  })

  it('displays rating with correct format', () => {
    const wrapper = mount(SolicitorCard, {
      props: { solicitor: mockSolicitor }
    })

    expect(wrapper.find('.rating-value').text()).toBe('4.5')
    expect(wrapper.find('.review-count').text()).toContain('120 reviews')
  })

  it('generates correct star rating', () => {
    const wrapper = mount(SolicitorCard, {
      props: { solicitor: mockSolicitor }
    })

    const stars = wrapper.find('.stars').text()
    expect(stars).toContain('★')
    expect(stars.length).toBeGreaterThan(0)
  })

  it('displays location when provided', () => {
    const wrapper = mount(SolicitorCard, {
      props: { solicitor: mockSolicitor }
    })

    expect(wrapper.find('.location').exists()).toBe(true)
    expect(wrapper.find('.location').text()).toContain('London, UK')
  })

  it('hides location when not provided', () => {
    const solicitorWithoutLocation = { ...mockSolicitor, location: null }
    const wrapper = mount(SolicitorCard, {
      props: { solicitor: solicitorWithoutLocation }
    })

    expect(wrapper.find('.location').exists()).toBe(false)
  })

  it('displays phone number with tel link', () => {
    const wrapper = mount(SolicitorCard, {
      props: { solicitor: mockSolicitor }
    })

    const phoneLink = wrapper.find('a[href^="tel:"]')
    expect(phoneLink.exists()).toBe(true)
    expect(phoneLink.attributes('href')).toBe('tel:+44 20 1234 5678')
  })

  it('displays website with correct attributes', () => {
    const wrapper = mount(SolicitorCard, {
      props: { solicitor: mockSolicitor }
    })

    const websiteLink = wrapper.find('a[target="_blank"]')
    expect(websiteLink.exists()).toBe(true)
    expect(websiteLink.attributes('href')).toBe('https://example.com')
    expect(websiteLink.attributes('rel')).toBe('noopener')
  })

  it('displays description when provided', () => {
    const wrapper = mount(SolicitorCard, {
      props: { solicitor: mockSolicitor }
    })

    expect(wrapper.find('.description').exists()).toBe(true)
    expect(wrapper.find('.description').text()).toBe('A professional solicitor firm')
  })

  it('formats date correctly', () => {
    const wrapper = mount(SolicitorCard, {
      props: { solicitor: mockSolicitor }
    })

    const dateText = wrapper.find('.date').text()
    expect(dateText).toContain('Updated:')
  })

  it('getStars method returns correct stars for whole numbers', () => {
    const wrapper = mount(SolicitorCard, {
      props: { solicitor: { ...mockSolicitor, ratingStars: 5 } }
    })

    const result = wrapper.vm.getStars(5)
    expect(result).toBe('★★★★★')
  })

  it('getStars method returns correct stars with half star', () => {
    const wrapper = mount(SolicitorCard, {
      props: { solicitor: mockSolicitor }
    })

    const result = wrapper.vm.getStars(4.5)
    expect(result).toContain('★★★★')
    expect(result).toContain('½')
  })

  it('getStars method returns correct stars with empty stars', () => {
    const wrapper = mount(SolicitorCard, {
      props: { solicitor: { ...mockSolicitor, ratingStars: 2 } }
    })

    const result = wrapper.vm.getStars(2)
    expect(result).toContain('★★')
    expect(result).toContain('☆')
  })
})
