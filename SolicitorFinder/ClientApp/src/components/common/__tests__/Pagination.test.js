import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import Pagination from '../Pagination.vue'

describe('Pagination', () => {
  it('renders pagination buttons correctly', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 1,
        totalPages: 5,
        totalCount: 50
      }
    })

    expect(wrapper.findAll('.pagination-btn').length).toBeGreaterThan(0)
  })

  it('does not render when totalPages is 1 or less', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 1,
        totalPages: 1,
        totalCount: 10
      }
    })

    expect(wrapper.find('.pagination').exists()).toBe(false)
  })

  it('disables previous button on first page', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 1,
        totalPages: 5,
        totalCount: 50
      }
    })

    const prevButton = wrapper.findAll('.pagination-btn')[0]
    expect(prevButton.attributes('disabled')).toBeDefined()
  })

  it('disables next button on last page', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 5,
        totalPages: 5,
        totalCount: 50
      }
    })

    const buttons = wrapper.findAll('.pagination-btn')
    const nextButton = buttons[buttons.length - 1]
    expect(nextButton.attributes('disabled')).toBeDefined()
  })

  it('enables previous button when not on first page', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 3,
        totalPages: 5,
        totalCount: 50
      }
    })

    const prevButton = wrapper.findAll('.pagination-btn')[0]
    expect(prevButton.attributes('disabled')).toBeUndefined()
  })

  it('enables next button when not on last page', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 3,
        totalPages: 5,
        totalCount: 50
      }
    })

    const buttons = wrapper.findAll('.pagination-btn')
    const nextButton = buttons[buttons.length - 1]
    expect(nextButton.attributes('disabled')).toBeUndefined()
  })

  it('emits page-change event when clicking a page number', async () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 1,
        totalPages: 5,
        totalCount: 50
      }
    })

    const pageButtons = wrapper.findAll('.pagination-btn').filter(btn =>
      !btn.text().includes('◀') && !btn.text().includes('▶')
    )

    await pageButtons[1].trigger('click')

    expect(wrapper.emitted('page-change')).toBeTruthy()
    expect(wrapper.emitted('page-change')[0]).toEqual([2])
  })

  it('emits page-change event when clicking next button', async () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 2,
        totalPages: 5,
        totalCount: 50
      }
    })

    const buttons = wrapper.findAll('.pagination-btn')
    const nextButton = buttons[buttons.length - 1]

    await nextButton.trigger('click')

    expect(wrapper.emitted('page-change')).toBeTruthy()
    expect(wrapper.emitted('page-change')[0]).toEqual([3])
  })

  it('emits page-change event when clicking previous button', async () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 3,
        totalPages: 5,
        totalCount: 50
      }
    })

    const prevButton = wrapper.findAll('.pagination-btn')[0]

    await prevButton.trigger('click')

    expect(wrapper.emitted('page-change')).toBeTruthy()
    expect(wrapper.emitted('page-change')[0]).toEqual([2])
  })

  it('displays pagination info correctly', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 3,
        totalPages: 10,
        totalCount: 100
      }
    })

    const info = wrapper.find('.pagination-info').text()
    expect(info).toContain('100 Items')
    expect(info).toContain('Page 3')
    expect(info).toContain('10')
  })

  it('marks current page as active', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 3,
        totalPages: 5,
        totalCount: 50
      }
    })

    const pageButtons = wrapper.findAll('.pagination-btn').filter(btn =>
      !btn.text().includes('◀') && !btn.text().includes('▶')
    )

    const activeButton = pageButtons.find(btn => btn.classes().includes('active'))
    expect(activeButton).toBeTruthy()
    expect(activeButton.text()).toBe('3')
  })

  it('computes hasPrevious correctly', () => {
    const wrapper1 = mount(Pagination, {
      props: { currentPage: 1, totalPages: 5, totalCount: 50 }
    })
    expect(wrapper1.vm.hasPrevious).toBe(false)

    const wrapper2 = mount(Pagination, {
      props: { currentPage: 2, totalPages: 5, totalCount: 50 }
    })
    expect(wrapper2.vm.hasPrevious).toBe(true)
  })

  it('computes hasNext correctly', () => {
    const wrapper1 = mount(Pagination, {
      props: { currentPage: 5, totalPages: 5, totalCount: 50 }
    })
    expect(wrapper1.vm.hasNext).toBe(false)

    const wrapper2 = mount(Pagination, {
      props: { currentPage: 4, totalPages: 5, totalCount: 50 }
    })
    expect(wrapper2.vm.hasNext).toBe(true)
  })

  it('limits visible pages based on maxVisible prop', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 5,
        totalPages: 20,
        totalCount: 200,
        maxVisible: 3
      }
    })

    expect(wrapper.vm.visiblePages.length).toBeLessThanOrEqual(3)
  })
})
